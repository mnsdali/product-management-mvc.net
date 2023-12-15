using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MiniProjet_.NET.Models;
using MiniProjet_.NET.Models.Repositories;
using Net.Codecrete.QrCodeGenerator;
using System.Globalization;

namespace MiniProjet_.NET.Controllers
{
    public class VariationController : Controller
    {
        //Injection de la dépendance
        readonly IVariationRepository VariationRepository;
        readonly IProduitRepository ProduitRepository;
        readonly IPieceRepository PieceRepository;
        readonly IArticleRepository ArticleRepository;
        readonly IPieceVariationRepository PieceVariationRepository;
        private readonly IWebHostEnvironment hostingEnvironment;


        public VariationController(IVariationRepository VariationRepository, IProduitRepository ProduitRepository,
                IPieceRepository PieceRepository, 
                 IWebHostEnvironment hostingEnvironment, IArticleRepository articleRepository, IPieceVariationRepository PieceVariationRepository
                 )
        {
            this.VariationRepository = VariationRepository;
            this.ProduitRepository = ProduitRepository;
            this.PieceRepository = PieceRepository;
            this.hostingEnvironment = hostingEnvironment;
            ArticleRepository = articleRepository;
            this.PieceVariationRepository = PieceVariationRepository;
        }
        public string GenerateSerieNumber()
        {
            var rand = new Random();
            string serialNumber;
            while (true)
            {
                serialNumber = "";

                for (int i = 0; i < 13; i++)
                {
                    serialNumber += rand.Next(0, 10).ToString();
                }

                var article = ArticleRepository.GetArticleBySerieNumber(serialNumber);

                if (article == null)
                {
                    break;
                }
            }

            return serialNumber;
        }

        public void GenerateQrCode(string serieNumber, string designation)
        {
            // Encode the text to a QR code
            var qr = QrCode.EncodeText(serieNumber, QrCode.Ecc.Medium);

            // Convert the QR code to SVG string
            string svg = qr.ToSvgString(4);


            // Specify the folder to save the QR code SVG file
            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, $"QrCodes/Articles/{designation}");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            // Ensure the folder exists
            Directory.CreateDirectory(uploadsFolder);

            // Specify the file path
            string uniqueFileName = $"{serieNumber}_serieNumber.svg";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the SVG content to the file
            System.IO.File.WriteAllText(filePath, svg);
        }
        // GET: VariationController
        public ActionResult Index()
        {
            

            return View();
        }

        // GET: VariationController/Details/5
        public ActionResult Details(int id)
        {
            var variation = VariationRepository.GetVariation(id);
            Console.WriteLine(variation.Articles.Count());
            return View(variation);
        }

        public ActionResult PrintAllQrs(int id)
        {
            var variation = VariationRepository.GetVariation(id);

            
            return View(variation);
            
        }

        public ActionResult Create()
        {
            var model = (ProduitRepository.GetProduits(), PieceRepository.GetPieces());

            return View(model);
        }

        // POST: VariationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            var id = collection["referenceId"];
           


            var designations = collection["designations[]"];
            var quantities = collection["quantities[]"];

            // Assuming you know the number of variations
            var numberOfVariations = int.Parse(collection["numberOfVariations"]);
            var pieceIds = new List<List<int>>();
            for (int i = 0; i < numberOfVariations; i++)
            {
                if (!StringValues.IsNullOrEmpty(collection[$"referencesPieces[{i}][]"]))
                {
                    var variationReferences = collection[$"referencesPieces[{i}][]"];
                    var referencesPieces = variationReferences.Select(int.Parse).ToList();
                    pieceIds.Add(referencesPieces);
                }
            }
            Console.WriteLine(pieceIds);

            var designationsList = designations.ToList();
            var quantitiesList = quantities.ToList();


            Produit produit = ProduitRepository.GetProduit(int.Parse(id));
            

            for (int i = 0; i < designations.Count(); i++)
            {
                Variation variation = VariationRepository.GetVariationByDesignation(designations[i]);
                if (variation == null)
                {
                    Variation newVariation = new()
                    {
                        Designation = designations[i],
                        ProduitId = produit.Id
                    };
                    VariationRepository.AddVariation(newVariation);
                    for (int j = 0; j < int.Parse(quantities[i]); j++)
                    {
                        var serieNumber = GenerateSerieNumber();
                        Article newArticle = new()
                        {
                            SerieNumber = serieNumber,
                            Status = false,
                            Variation = newVariation
                        };
                        ArticleRepository.AddArticle(newArticle);
                        GenerateQrCode(serieNumber, designations[i]);
                    }

                    foreach (var PieceId in pieceIds[i])
                    {
                        PieceVariation newPieceVariation = PieceVariationRepository.GetPieceVariationByIds(PieceId, newVariation.Id);
                        if (newPieceVariation == null)
                        {
                            newPieceVariation = new()
                            {
                                Variation = newVariation,
                                Piece = PieceRepository.GetPiece(PieceId)
                            };
                            PieceVariationRepository.AddPieceVariation(newPieceVariation);
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

        // GET: VariationController/Edit/5
        public ActionResult Edit(int id)
        {
            // variations.add-pieces
            ViewData["Variations"] = VariationRepository.GetVariations();
            ViewData["Pieces"] = PieceRepository.GetPieces();

            return View();
        }

        // POST: VariationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VariationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VariationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private bool ValidateVarCreation(IFormCollection collection)
        {
            if (string.IsNullOrEmpty(collection["referenceProd"]) ||
                !collection.ContainsKey("designations") ||
                !collection.ContainsKey("quantities"))
            {
                // Add validation errors to the ModelState
                ModelState.AddModelError(string.Empty, "Validation failed. Please provide all required data.");
                return false;
            }

            var designations = collection["designations"];
            var quantities = collection["quantities"];

            if (designations.Count != quantities.Count)
            {
                ModelState.AddModelError(string.Empty, "The number of designations must match the number of quantities.");
                return false;
            }

            for (int i = 0; i < designations.Count; i++)
            {
                if (string.IsNullOrEmpty(designations[i]))
                {
                    ModelState.AddModelError($"designations[{i}]", "Designation is required.");
                }

                if (!int.TryParse(quantities[i], out _))
                {
                    ModelState.AddModelError($"quantities[{i}]", "Quantity must be an integer.");
                }
            }

            return ModelState.IsValid; // Returns true if there are no validation errors
        }

        private bool ValidatePieceSelect(IFormCollection collection)
        {
            if (!collection.ContainsKey("referencesPieces"))
            {
                // Add validation errors to the ModelState
                ModelState.AddModelError("referencesPieces", "ReferencesPieces is required.");
                return false;
            }

            var referencesPieces = collection["referencesPieces"];

            for (int i = 0; i < referencesPieces.Count; i++)
            {
                if (string.IsNullOrEmpty(referencesPieces[i]))
                {
                    ModelState.AddModelError($"referencesPieces[{i}]", "Reference piece is required.");
                }
            }

            return ModelState.IsValid; // Returns true if there are no validation errors
        }


    }
}
